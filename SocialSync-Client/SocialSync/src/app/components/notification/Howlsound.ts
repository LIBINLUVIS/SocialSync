import { Injectable } from "@angular/core";
import { Howl } from 'howler';
@Injectable({
  "providedIn": 'root'
})

export default class Howlsound {
    private successSound: Howl;
    private errorSound: Howl;

    constructor() {
        this.successSound = new Howl({
            src: ['assets/linkedin_notification.mp3'], 
            volume: 0.5,
          });
        this.errorSound = new Howl({
            src: ['assets/linked_in_toast_red.mp3'], 
            volume: 1,
          });
    }
    
    playSuccess() {
  
        this.successSound.play();
      }
    
    playError() {
        this.errorSound.play();
      }

}