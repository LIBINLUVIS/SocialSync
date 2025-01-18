import { Component, ViewChild } from '@angular/core';
import { HeaderComponent } from '../../../Layouts/header/header.component';
import { ApiSocialService } from '../../../core/services/api.services/api.social.service';
import { NgxLoadingModule } from 'ngx-loading';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../../core/services/AuthServices/AuthService';

@Component({
  selector: 'app-my-schedulers',
  standalone: true,
  imports: [HeaderComponent,NgxLoadingModule,CommonModule],
  templateUrl: './my-schedulers.component.html',
  styleUrl: './my-schedulers.component.scss'
})
export class MySchedulersComponent {
  @ViewChild('headerComp', { static: false }) headerComp!: HeaderComponent;
  userName:string = "";
  userId:number = 1;
  public loading = true;
  pagename:string = "";
  commentary:string ="";
  scheduledDate:any = "";
  provider:string = "";
  isPosted:boolean = false;
  myschedules: { pagename:string,commentary:string,scheduledDate:string,provider:string,isPosted:boolean }[] = [];
  constructor(private socialService:ApiSocialService,private authService:AuthService){}
  ngOnInit(){
    var userName = this.authService.getUserProfile();
    if(userName){
      this.userName = userName
    }else{
      this.userName = ""
    }
    this.truncateText('.content-text',50)
    this.getmySchedules();
  }

  truncateText(selector:any, maxLength:number) {
    const element = document.querySelector(selector);
    if (element && element.textContent.length > maxLength) {
        element.textContent = element.textContent.substring(0, maxLength) + '...';
    }
  }
  getmySchedules(){
    this.socialService.getmySchedules(this.userId).subscribe({
       next:(success)=>{
        if(success.statusCode==200){
          this.loading = false;
          success.data.forEach((res:any)=>{
            this.myschedules.push({pagename:res.pagename,commentary:res.commentary,
              scheduledDate:res.scheduledDate,provider:res.provider,isPosted:res.isPosted})
          })
          
        }
       },
       error:(err)=>{
        this.loading = false;
       }
    })
  }
}
