import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { OtpTimerComponent } from './components/otp-timer/OTPTimer/otp-timer.component';
import { ApiAccountService } from './core/services/api.services/api.account.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet,OtpTimerComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
})
export class AppComponent {
  title = 'SocialSync';
  
  constructor(private http:ApiAccountService){}
  ngOnInit(){
    this.http.test().subscribe(
      success =>{
        console.log(success);
      }
    )
  }
}
