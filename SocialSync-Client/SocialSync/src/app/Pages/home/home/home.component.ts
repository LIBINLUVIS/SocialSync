import { Component } from '@angular/core';
import { AuthService } from '../../../core/services/AuthServices/AuthService';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './home.component.html',
  styleUrl: './home.component.scss'
})
export class HomeComponent {
 
 isUserAuth:boolean = false;

 constructor(
  private authService:AuthService,
  private router:Router

 ){}

 ngOnInit(){
   
   this.authService.loadTokenFromSession();

   this.isUserAuth = this.authService.isAuthenticated();

   if(!this.isUserAuth){
    this.router.navigate(['login'])
   }

 }
}
