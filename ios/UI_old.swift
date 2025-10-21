//
//  UI.swift
//  Symbios
//
//  Created by Joakim Wennergren on 2023-08-29.
//

import Foundation
import UIKit

import AVFoundation

@MainActor
var player: AVAudioPlayer?

@MainActor
var player2: AVAudioPlayer?

@MainActor
var player3: AVAudioPlayer?

@MainActor @_cdecl("StopSound")
func stopSound(index: UInt32) {
    if(index == 1){
        player?.stop()
    }
    
    if(index == 2){
        player2?.stop()
    }
    
    if(index == 3){
        player3?.stop()
    }
}


@MainActor @_cdecl("PlaySound")
func playSound(index: UInt32) {
    
    var file = ""
        
    if (index == 1) {
        file = "click"
        guard let path = Bundle.main.path(forResource: file, ofType:"mp3") else {
            return
        }
        
        let url = URL(fileURLWithPath: path)

        do {
            player = try AVAudioPlayer(contentsOf: url)
            player?.play()
            
        } catch let error {
            print(error.localizedDescription)
        }
    }
    
    if(index == 2) {
        file = "Svampjakt_startscreen"
        guard let path = Bundle.main.path(forResource: file, ofType:"wav") else {
            return
        }
        
        let url = URL(fileURLWithPath: path)

        do {
            player2 = try AVAudioPlayer(contentsOf: url)
            player2?.volume = 0.6
            player2?.numberOfLoops = -1
            player2?.play()
            
        } catch let error {
            print(error.localizedDescription)
        }
    }
    
    if(index == 3) {
        file = "birdsong"
        guard let path = Bundle.main.path(forResource: file, ofType:"mp3") else {
            return
        }
        
        let url = URL(fileURLWithPath: path)

        do {
            player3 = try AVAudioPlayer(contentsOf: url)
            player3?.volume = 0.6
            player3?.numberOfLoops = -1
            player3?.play()
            
        } catch let error {
            print(error.localizedDescription)
        }
    }
    

}


@MainActor
var _view = UIView()
@MainActor
var _screen = UIScreen()
@MainActor
var _touchPoint = CGPoint()

class SecondViewController: UIViewController {
    
    override func didRotate(from fromInterfaceOrientation: UIInterfaceOrientation) {
        switch UIDevice.current.orientation{
        case .portrait: break
        case .portraitUpsideDown: break
        case .landscapeLeft:
            print("in landscape left")
            let temp = _view.frame.size.height
            _view.frame.size.height = temp + 30
        case .landscapeRight:
            print("in landscape left")
            let temp = _view.frame.size.height
            _view.frame.size.height = temp + 30
        default: break
        }
    }
    
    override func touchesBegan(_ touches: Set<UITouch>, with event: UIEvent?) {
        if let touch = touches.first {
            let currentPoint = touch.location(in: self.view)
            _touchPoint = currentPoint;
        }
    }

    override func touchesMoved(_ touches: Set<UITouch>, with event: UIEvent?) {
        if let touch = touches.first {
            //let currentPoint = touch.location(in: self.view)

        }
    }

    override func touchesEnded(_ touches: Set<UITouch>, with event: UIEvent?) {
        if let touch = touches.first {
            //let currentPoint = touch.location(in: self.view)
            _touchPoint = CGPoint(x: 0, y: 0)
        }
    }
    
    override func viewDidAppear(_ animated: Bool) {
        
        let orientation = UIDevice.current.orientation
        
        if(orientation == .portrait)
        {
            
        } else if (orientation == .landscapeLeft )
        {
            print("in landscape left")
            let temp = _view.frame.size.height
            _view.frame.size.height = temp + 30
        } else if(orientation == .landscapeRight)
        {
            print("in landscape right")
            let temp = _view.frame.size.height
            _view.frame.size.height = temp + 30
        }
        
        super.viewDidLoad()
    
    }
}

extension UIWindow {
    static var current: UIWindow? {
        for scene in UIApplication.shared.connectedScenes {
            guard let windowScene = scene as? UIWindowScene else { continue }
            for window in windowScene.windows {
                if window.isKeyWindow { return window }
            }
        }
        return nil
    }
}

extension UIScreen {
    static var current: UIScreen? {
        UIWindow.current?.screen
    }
}

@MainActor
@_cdecl("touch")
func touch() -> CGPoint
{
    return _touchPoint
}

@MainActor
@_cdecl("get_native_bounds")
func getNativeBounds(view: UIView, screen: UIScreen) -> UIViewController
{
    _view = view
    _screen = screen
    let controller = SecondViewController();
    
    return controller;
}

@MainActor
@_cdecl("get_native_scale")
func getNativeBounds() -> Float {
    return Float(UIScreen.current!.nativeScale)
}
