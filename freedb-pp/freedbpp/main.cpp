#define BOOST_FILESYSTEM_VERSION 3
#include <boost/filesystem.hpp>
#include <boost/format.hpp>
#include <boost/algorithm/string.hpp>
#include <boost/lexical_cast.hpp>
#include <boost/progress.hpp>
#include <ppl.h>
#include <concurrent_vector.h>
#include <iostream>
#include <fstream>
#include <unordered_map>

typedef std::unordered_map<int, int> Counts;

Counts count(const std::vector<std::wstring>& files)
{
   Counts map;

   Concurrency::concurrent_vector<int> vec;

   Concurrency::parallel_for_each(files.begin(), files.end(), [&vec](std::wstring file) {
      std::ifstream input(file);
      std::string line;
      while (std::getline(input, line)!=0) {
         if (line.size()>6 && boost::starts_with(line, "DYEAR=")) {
            try {
               int year = boost::lexical_cast<int>(line.substr(6));
               vec.push_back(year);
               break;
            }
            catch (boost::bad_lexical_cast&)
            {}
         }
      }
   });

   std::for_each(vec.begin(), vec.end(), [&map](int i) {
      map[i]++;
   });

   return map;
}

int main(int argc, char** argv)
{
   using namespace boost::filesystem;
   using namespace boost;

   try {
      if (argc<2 || !is_directory(argv[1])) throw std::runtime_error(str(format("Usage: %1% <directory>") % argv[0]));

      boost::progress_timer timer;

      std::vector<std::wstring> files;
      std::stack<path> paths;

      std::shared_ptr<progress_timer> enumeratingTimer(new progress_timer);
      std::cout << "Enumerating files..." << std::endl;

      paths.push(path(argv[1]));
      while (!paths.empty()) {

         path p = paths.top();
         paths.pop();

         std::for_each(directory_iterator(p), directory_iterator(), [&files, &paths](path sub) {
            if (is_regular_file(sub)) {
               files.push_back(sub.native());
            }
            else if (is_directory(sub)) {
               paths.push(sub);
            }
         });
      }
      enumeratingTimer.reset();

      std::cout << "Found " << files.size() << " files" << std::endl;
      std::cout << "Scanning files..." << std::endl;
      auto counts = count(files);
      Counts::value_type max = *std::max_element(counts.begin(), counts.end(), [](Counts::value_type left, Counts::value_type right) {
         return left.second < right.second;
      });
      std::cout << "Year " << max.first << ": " << max.second << std::endl;
   }
   catch (std::exception& ex) {
      std::cerr << ex.what() << std::endl;
   }
}
