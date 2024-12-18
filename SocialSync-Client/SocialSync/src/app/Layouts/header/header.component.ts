import { Component, ElementRef, Input, Renderer2, ViewChild, viewChild } from '@angular/core';
import { Router } from '@angular/router';

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
    private router:Router
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
    console.log("hello")
     this.router.navigate(['accounts'])
  }

}
