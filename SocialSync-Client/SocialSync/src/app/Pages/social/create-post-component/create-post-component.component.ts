import { Component, computed, ElementRef, Renderer2, signal,ChangeDetectionStrategy, ViewChild } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatSelectModule } from '@angular/material/select';
import { MatFormFieldModule } from '@angular/material/form-field';
import { NgFor,NgIf,NgClass } from '@angular/common'
import {MatDividerModule} from '@angular/material/divider';
import { Page } from '../../../core/Interfaces/CreatePost';
import {MatInputModule} from '@angular/material/input';
import {NgxMaterialTimepickerModule} from 'ngx-material-timepicker';
import {provideNativeDateAdapter} from '@angular/material/core';
import {MatDatepickerModule} from '@angular/material/datepicker';
import { HeaderComponent } from '../../../Layouts/header/header.component';
import { AuthService } from '../../../core/services/AuthServices/AuthService';


@Component({
  selector: 'app-create-post-component',
  standalone: true,
  providers: [provideNativeDateAdapter()],
  imports: [FormsModule,MatInputModule,NgxMaterialTimepickerModule
    ,MatFormFieldModule,NgFor,MatSelectModule,MatCheckboxModule,
    MatDividerModule,NgIf,NgClass,MatDatepickerModule,FormsModule,HeaderComponent
  ],
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './create-post-component.component.html',
  styleUrl: './create-post-component.component.scss'
})

export class CreatePostComponentComponent {
 
  showPages:boolean = false;
  pagechecked:boolean = false;
  showDatetimePicker:boolean = false;
  value:Date = new Date();
  currentHour = this.value.getHours();
  currentMinute = this.value.getMinutes();
  currentSecond = this.value.getSeconds();
  userName:string = "";

  formattedTime = `${this.currentHour % 12 || 12}:${this.currentMinute.toString().padStart(2, '0')} ${this.currentHour >= 12 ? 'PM' : 'AM'}`;
  @ViewChild('headerComp', { static: false }) headerComp!: HeaderComponent;
  constructor(private authService:AuthService){

  }
  ngOnInit(){
    var userName = this.authService.getUserProfile();
    if(userName){
     this.userName = userName
    }else{
     this.userName = ""
    }
  }
  


  selectedPages:any[]=[];
  pages: Page[] = [
    {
      id:1,
      name:'Page 1',
      status:false
    },
    {
      id:2,
      name:'Page 2',
      status:false
    },
    {
      id:3,
      name:'Page 3',
      status:false
    }
  ]
  tooglepages():void{
    this.showPages = !this.showPages
    console.log(this.pages)
  }

  pageselected(id:any,name:string):void{
   this.pagechecked = !this.pagechecked
   let page = {
      id:id,
      name:name,
      status:false,
   }
   let pageobj = this.selectedPages.find(x=>x.id == id)
   if(pageobj){
     this.selectedPages.splice(this.selectedPages.indexOf(pageobj),1)
     var updatepageobj = this.pages.find(p=>p.id===id);
     if(updatepageobj){
       updatepageobj.status = !updatepageobj.status
     }
   }else{
    this.selectedPages.push(page)
    var updatepageobj = this.pages.find(p=>p.id===id);
    if(updatepageobj){
      updatepageobj.status = !updatepageobj.status
    }
 
   }

}

Schedule(){
  this.showDatetimePicker = true;
}
           
CancelSchedule(){
  this.showDatetimePicker = false;
}

}
