import { ChangeDetectionStrategy, Component,Output,EventEmitter, Input,SimpleChanges,ViewChild } from '@angular/core';
import { CountdownComponent,CountdownEvent } from 'ngx-countdown';
// import { ViewCodeComponent } from './view-code.component';


@Component({
  selector: 'app-otp-timer',
  standalone: true,
  imports: [CountdownComponent],
  templateUrl:"./otp-timer.component.html"


})
export class OtpTimerComponent {
  @Output() messageEvent = new EventEmitter<boolean>();
  @Input() IstimerReset:boolean = false;

  @ViewChild('countdown', { static: false }) private countdown!: CountdownComponent;

   constructor(){}


   ngOnInit():void{
     
   }

   ngOnChanges(changes:SimpleChanges):void{
    if (changes['IstimerReset'] && changes['IstimerReset'].currentValue) {
      this.resetTimerLogic();
    }
   }

   onTimerFinished(e:CountdownEvent){
    if(e.action == 'done'){
     this.messageEvent.emit(true); 
    }
   }

   resetTimerLogic(){
    // this.timer.restart();
    this.countdown.restart();
   }

   

}
