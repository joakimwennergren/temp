#include "entropy.h"

using namespace Entropy::EntryPoints;

extern "C" {
/**
 * @brief Initializes the C# runtime environment.
 *
 * This function should be called once at application startup to initialize
 * the C# interop layer before any other C# functions are invoked.
 */
void CSharpStart();

/**
 * @brief Main update callback for the C# runtime.
 *
 * This function is called every frame to update the C# application state.
 *
 * @param delta_time The time elapsed since the last frame, in seconds.
 * @param screen_width The current width of the screen/window in pixels.
 * @param screen_height The current height of the screen/window in pixels.
 */
void CSharpProgress(float delta_time);

/**
 * @brief Updates the mouse cursor position in the C# runtime.
 *
 * @param x The x-coordinate of the mouse position.
 * @param y The y-coordinate of the mouse position.
 */
void UpdateMousePosition(float x, float y);
}

int main() {
  auto engine = EntropyEngine(1200, 840);
  engine.OnCsharpStart = CSharpStart;
  engine.OnCsharpProgress = CSharpProgress;
  engine.Run();
  return EXIT_SUCCESS;
}
