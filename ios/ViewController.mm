#import <Metal/Metal.h>
#import <UIKit/UIkit.h>
#import "ViewController.h"
#import <CoreHaptics/CoreHaptics.h>
#import <Foundation/Foundation.h>
#import <AudioToolbox/AudioServices.h>
#include "entropy.h"

extern "C" {
void CSharpMain();
void CSharpOnUpdate(float delta_time, int32_t screen_width,
                    int32_t screen_height);
void UpdateMousePosition(float x, float y, bool is_down);
}

// Returns true if Low Power Mode is enabled
bool isLowPowerModeOn() {
    return [[NSProcessInfo processInfo] isLowPowerModeEnabled];
}

@interface ViewController ()

@end

@implementation ViewController

CAMetalLayer *_metalLayer;
GameView *_gameView;
Entropy::EntryPoints::EntropyEngine* entropy_engine;

- (void)touchesBegan:(NSSet<UITouch *> *)touches withEvent:(UIEvent *)event {
    UITouch *touch = [touches anyObject];
    CGPoint point = [touch locationInView:self.view]; // point in controller's view
    CGFloat screenScale = [UIScreen mainScreen].scale;
    CGRect screenBounds = [UIScreen mainScreen].bounds;
    UpdateMousePosition(point.x * screenScale, (screenBounds.size.height * screenScale)- point.y * screenScale, true);

    //NSLog(@"Touch Began at (%.2f, %.2f)", point.x * screenScale, (screenBounds.size.height * screenScale) - point.y * screenScale);
}

- (void)touchesMoved:(NSSet<UITouch *> *)touches withEvent:(UIEvent *)event {
    UITouch *touch = [touches anyObject];
    CGPoint point = [touch locationInView:self.view];
}

- (void)touchesEnded:(NSSet<UITouch *> *)touches withEvent:(UIEvent *)event {
    UITouch *touch = [touches anyObject];
    CGPoint point = [touch locationInView:self.view];
    CGFloat screenScale = [UIScreen mainScreen].scale;
    UpdateMousePosition(point.x * screenScale, point.y * screenScale, false);
}

- (void)loadView {
    // Create your GameView instead of the default UIView
    _gameView = [[GameView alloc] initWithFrame:[[UIScreen mainScreen] bounds]];
    self.view = _gameView;
}

- (void)viewDidLoad {
    [super viewDidLoad];
        
    // Setup CAMetalLayer
    _metalLayer = (CAMetalLayer *)_gameView.layer;
    //_metalLayer.device = MTLCreateSystemDefaultDevice();
    _metalLayer.pixelFormat = MTLPixelFormatBGRA8Unorm;
    _metalLayer.contentsScale = [UIScreen mainScreen].scale;
    
    // Get the main screen bounds
    CGRect screenBounds = [UIScreen mainScreen].bounds;

    // Get the screen scale factor (1.0 for non-Retina displays, 2.0 or 3.0 for Retina displays)
    CGFloat screenScale = [UIScreen mainScreen].scale;
    
    uint32_t width = screenBounds.size.width * screenScale;
    uint32_t height = screenBounds.size.height * screenScale;
    entropy_engine = new Entropy::EntryPoints::EntropyEngine((__bridge void*)_metalLayer, width, height);
    
    CSharpMain();

    if (isLowPowerModeOn()) {
        printf("⚠️ Low Power Mode is ON\n");
    } else {
        printf("✅ Low Power Mode is OFF\n");
    }
            
    // Set up the CADisplayLink
    self.displayLink = [CADisplayLink displayLinkWithTarget:self selector:@selector(update)];
    // iOS 15+ only:
    if ([self.displayLink respondsToSelector:@selector(setPreferredFrameRateRange:)]) {
        self.displayLink.preferredFrameRateRange = CAFrameRateRangeMake(60, 60, 60);
    } else {
        // Fallback for older iOS versions
        self.displayLink.preferredFramesPerSecond = 60;
    }
    [self.displayLink addToRunLoop:[NSRunLoop mainRunLoop] forMode:NSRunLoopCommonModes];
}

// This method will be called every frame
- (void)update {
    // Call the render function of your MetalView
    [_gameView render];
}

- (void)dealloc {
    // Clean up the display link when the view controller is deallocated
    [self.displayLink invalidate];
}

@end

@implementation GameView

/** Returns a Metal-compatible layer. */
+(Class) layerClass { return [CAMetalLayer class]; }

- (void)render {
    if(entropy_engine) {
        // Get the main screen bounds
        CGRect screenBounds = [UIScreen mainScreen].bounds;
        // Get the screen scale factor (1.0 for non-Retina displays, 2.0 or 3.0 for Retina displays)
        CGFloat screenScale = [UIScreen mainScreen].scale;
        uint32_t width = screenBounds.size.width * screenScale;
        uint32_t height = screenBounds.size.height * screenScale;
        entropy_engine->Run([&](float deltaTime, int32_t screenWidth, int32_t screenHeight) {
            CSharpOnUpdate(deltaTime, screenWidth, screenHeight);
        }, width, height, 0);
        entropy_engine->renderer->Render(width, height);
    }
}

@end
