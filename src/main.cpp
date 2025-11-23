#include "entropy.h"

using namespace Entropy::EntryPoints;

extern "C" {
  void CSharpStart();
  void CSharpProgress(float delta_time);
  void CSharpOnUpdate(float delta_time, int32_t screen_width,
                      int32_t screen_height);
  void UpdateMousePosition(float x, float y);
}

int main() {
  auto engine = EntropyEngine(1024, 640);
  engine.OnCsharpStart = CSharpStart;
  engine.OnCsharpProgress = CSharpProgress;
  engine.Run();
  return EXIT_SUCCESS;
}