#include "entropy.h"

#include "assets/iasset_manager.h"
#include "assets/texture_asset.h"

extern "C" {
void CSharpMain();
void CSharpOnUpdate(float delta_time, int32_t screen_width,
                    int32_t screen_height);
}

int main() {
  auto engine = Entropy::EntryPoints::EntropyEngine(1024, 640);
  CSharpMain();
  engine.Run([](float delta_time, int32_t screen_width,
                     int32_t screen_height) {
    CSharpOnUpdate(delta_time, screen_width, screen_height);
  });
  return EXIT_SUCCESS;
}