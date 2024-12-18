import { Component, OnInit, ViewChild } from '@angular/core';
import { AuthService } from '../../../core/services/AuthServices/AuthService';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { HeaderComponent } from '../../../Layouts/header/header.component';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule,HeaderComponent],
  templateUrl: './home.component.html',
  styleUrl: './home.component.scss'
})
export class HomeComponent implements OnInit{
 
 isUserAuth:boolean = false;
 userName:string = "";
 text: string = 'Welcome to Your Ultimate Social Media Management Tool! 🌟';
 startText:string = 'SocialSync 🚀' 
 newtext:string = '';
 displayedText: string = '';    
 typingSpeed: number = 80;
 isTextComplete: boolean = false;
 isnextTextComplete :boolean = false;

 @ViewChild('headerComp', { static: false }) headerComp!: HeaderComponent;

 constructor(
  private authService:AuthService,
  private router:Router

 ){}

 ngOnInit(){
   
 var userName = this.authService.getUserProfile();
 if(userName){
  this.userName = userName
 }else{
  this.userName = ""
 }

 this.startTyping();


 }


 startTyping() {
  let index = 0;
  const interval = setInterval(() => {
    if (index < this.text.length) {
      this.displayedText += this.text.charAt(index);
      index++;
    } else {
      this.isTextComplete = true;
      clearInterval(interval);
      this.startTypeingnxtText() // Stop interval when done
    }
  }, this.typingSpeed);


}

startTypeingnxtText(){
  let index = 0;
  const interval = setInterval(() => {
    if (index < this.startText.length) {
      this.newtext += this.startText.charAt(index);
      index++;
    } else {
      this.isnextTextComplete = true;
      clearInterval(interval); // Stop interval when done
    }
  }, this.typingSpeed);
}

onButtonClick() {
  this.headerComp.openOffcanvas();
}

}



