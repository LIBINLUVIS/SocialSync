import { Component } from '@angular/core';
import { HeaderComponent } from '../../../Layouts/header/header.component';

@Component({
  selector: 'app-my-schedulers',
  standalone: true,
  imports: [HeaderComponent],
  templateUrl: './my-schedulers.component.html',
  styleUrl: './my-schedulers.component.scss'
})
export class MySchedulersComponent {
  userName:string = "Libin";

  constructor(){}
  ngOnInit(){
    this.truncateText('.content-text',50)
  }

  truncateText(selector:any, maxLength:number) {
    const element = document.querySelector(selector);
    if (element && element.textContent.length > maxLength) {
        element.textContent = element.textContent.substring(0, maxLength) + '...';
    }
  }
}
