import { ChangeDetectionStrategy, Component,Output,EventEmitter } from '@angular/core';
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

   constructor(){}


   onTimerFinished(e:CountdownEvent){
    if(e.action == 'done'){
     this.messageEvent.emit(true); 
    }
   }

}
