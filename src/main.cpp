#include "entropy.h"

int main() {
  auto engine = Entropy::EntryPoints::EntropyEngine(1024, 640);
  engine.Run();
  return EXIT_SUCCESS;
}