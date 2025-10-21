#import <UIKit/UIKit.h>

/** The Metal-compatibile view for the demo Storyboard. */
@interface GameView : UIView
- (void)render;
@end

@interface ViewController : UIViewController

@property (nonatomic, strong) CADisplayLink *displayLink;

@end


