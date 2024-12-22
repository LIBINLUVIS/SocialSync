import { Component, ViewChild } from '@angular/core';
import { Router, RouterOutlet } from '@angular/router';
import { OtpTimerComponent } from './components/otp-timer/OTPTimer/otp-timer.component';
import { ApiAccountService } from './core/services/api.services/api.account.service';
import { AuthService } from './core/services/AuthServices/AuthService';
import { HeaderComponent } from './Layouts/header/header.component';


@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet,OtpTimerComponent,HeaderComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
})
export class AppComponent {
  title = 'SocialSync';
  userName:string = "";
  isUserAuth:boolean = false;

  @ViewChild('headerComp', { static: false }) headerComp!: HeaderComponent;

  constructor(private authService:AuthService,private router:Router){}

  ngOnInit(){

    this.authService.loadTokenFromSession();
   
    this.isUserAuth = this.authService.isAuthenticated();
 
    if(!this.isUserAuth){
     this.router.navigate(['login'])
    }

    var userName = this.authService.getUserProfile();
    if(userName){
     this.userName = userName
    }else{
     this.userName = ""
    }
    
  }
}
