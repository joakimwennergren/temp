#include "entropy.h"

#include "assets/iasset_manager.h"
#include "assets/texture_asset.h"

extern "C" {
void CSharpMain();
void CSharpOnUpdate(float delta_time, int32_t screen_width,
                    int32_t screen_height);
void UpdateMousePosition(float x, float y);
}

int main() {
  auto engine = Entropy::EntryPoints::EntropyEngine(1024, 640);
  engine.MouseUpdate = UpdateMousePosition;
  engine.OnUpdate = CSharpOnUpdate;
  CSharpMain();
  engine.Run();
  return EXIT_SUCCESS;
}