import { Component } from '@angular/core';
import { Router, RouterOutlet } from '@angular/router';
import { OtpTimerComponent } from './components/otp-timer/OTPTimer/otp-timer.component';
import { ApiAccountService } from './core/services/api.services/api.account.service';
import { AuthService } from './core/services/AuthServices/AuthService';


@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet,OtpTimerComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
})
export class AppComponent {
  title = 'SocialSync';

  isUserAuth:boolean = false;

  constructor(private authService:AuthService,private router:Router){}

  ngOnInit(){

    this.authService.loadTokenFromSession();
   
    this.isUserAuth = this.authService.isAuthenticated();
 
    if(!this.isUserAuth){
     this.router.navigate(['login'])
    }
  }
}
