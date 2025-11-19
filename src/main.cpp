#include "entropy.h"

using namespace Entropy::EntryPoints;

int main() {
  auto engine = EntropyEngine(1024, 640);
  engine.Run();
  return EXIT_SUCCESS;
}