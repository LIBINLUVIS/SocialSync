import { Component, ElementRef, Input, Renderer2, ViewChild, viewChild } from '@angular/core';
import { Router } from '@angular/router';
import ApistorageService from '../../core/services/localstorageService/api,storage.service';

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [],
  templateUrl: './header.component.html',
  styleUrl: './header.component.scss'
})
export class HeaderComponent {

  @Input() userName:string = ""
  @ViewChild('navbarTogglerButton',{ static: false }) navbarTogglerButton!: ElementRef;
  isDarkTheme = false;
  
  constructor(
    private renderer:Renderer2,
    private router:Router,
    private localstorageService:ApistorageService
  ){

  }

  switchMode(){
    this.isDarkTheme = !this.isDarkTheme;
    const themeClass = this.isDarkTheme ? 'dark-theme' : 'light-theme';
    this.renderer.setAttribute(document.body,'class',themeClass);

  }

  public openOffcanvas(): void {
    this.navbarTogglerButton.nativeElement.click(); 
  }

  NavtoSocialaccounts(){
     this.router.navigate(['accounts'])
  }
  NavtoCreatePost(){
    this.router.navigate(['CreatePost'])
  }
  NavtoMyPage(){
    this.router.navigate(['MyPage'])
  }
  NavtoMySchedules(){
    this.router.navigate(['MySchedules'])
  }
  logout(){
     this.localstorageService.clearStorage();
     this.router.navigate(['login'])
  }

}
