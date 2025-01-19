import { Component, computed, ElementRef, Renderer2, signal,ChangeDetectionStrategy, ViewChild, ChangeDetectorRef, Signal, WritableSignal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatSelectModule } from '@angular/material/select';
import { MatFormFieldModule } from '@angular/material/form-field';
import { NgFor,NgIf,NgClass, DatePipe } from '@angular/common'
import {MatDividerModule} from '@angular/material/divider';
import { Page } from '../../../core/Interfaces/CreatePost';
import {MatInputModule} from '@angular/material/input';
import {NgxMaterialTimepickerModule} from 'ngx-material-timepicker';
import {provideNativeDateAdapter} from '@angular/material/core';
import {MatDatepickerModule} from '@angular/material/datepicker';
import { HeaderComponent } from '../../../Layouts/header/header.component';
import { AuthService } from '../../../core/services/AuthServices/AuthService';
import { ApiSocialService } from '../../../core/services/api.services/api.social.service';
import { ApiAccountService } from '../../../core/services/api.services/api.account.service';
import { Router } from '@angular/router';
import { MatSnackBar } from '@angular/material/snack-bar';
import ApistorageService from '../../../core/services/localstorageService/api,storage.service';
import Howlsound from '../../../components/notification/Howlsound';
import { DateTime } from 'luxon';



@Component({
  selector: 'app-create-post-component',
  standalone: true,
  providers: [provideNativeDateAdapter()],
  imports: [FormsModule,MatInputModule,NgxMaterialTimepickerModule
    ,MatFormFieldModule,NgFor,MatSelectModule,MatCheckboxModule,
    MatDividerModule,NgIf,NgClass,MatDatepickerModule,FormsModule,HeaderComponent,DatePipe
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
  socialAccounts = ['LinkedIn', 'Instagram', 'Twitter'];
  selectedOption = 'Twitter';
  UserSelectedroviderAccessToken:string = "";
  formattedTime = `${this.currentHour % 12 || 12}:${this.currentMinute.toString().padStart(2, '0')} ${this.currentHour >= 12 ? 'PM' : 'AM'}`;
  @ViewChild('headerComp', { static: false }) headerComp!: HeaderComponent;
  public isadminpageloading: WritableSignal<boolean> = signal(true);
  posttextValue: string = "";
  attachedFile:File | null = null;
  attachedFileValue:string = "";
  userAccountId:string = "";
  IsPostScheduled:boolean = false;

  constructor(private authService:AuthService,private apiSocialService:ApiSocialService,
    private apiaccountService:ApiAccountService,private router:Router,
    private snackBar: MatSnackBar,private storageService:ApistorageService,
    private notificationSound:Howlsound,private cdt:ChangeDetectorRef) {} 


  ngOnInit(){
    var userName = this.authService.getUserProfile();
    if(userName){
     this.userName = userName
    }else{
     this.userName = ""
    }
   this.userAccountId = sessionStorage.getItem('Accountid')?.toString() || "" 
   if(this.userAccountId==null){
     this.ErrorSnackBar('Error in getting the user Account Id');
   }


  }
  
  ErrorSnackBar(message: string) {
    this.snackBar.open(message, '❌', {
      verticalPosition: 'bottom',
      horizontalPosition: 'center',
      panelClass: 'custom-snackbar',
    });
  }

  SuccessSnackBar(message: string) {
    this.snackBar.open(message, 'close', {
      duration: 2000, 
      verticalPosition: 'bottom',
      horizontalPosition: 'center',
      panelClass: ['snackbar-global'],
    });
  }

  selectSocialAccount(){
    var userid = sessionStorage.getItem('Userid')
    if(this.selectedOption=="LinkedIn" && userid!=null){
      //checking the authtoken is valid or not
      this.apiaccountService.getConnectionStatus(userid).subscribe({
        next:(success)=>{
        
          if(success[0].Provider =="Linkedin" && success[0].IsActive == 'false'){
             this.router.navigate(['accounts'])
          }else{
            this.UserSelectedroviderAccessToken = this.storageService.get('LinkedinAccesstoken') || ""
            this.getAdminPages(this.UserSelectedroviderAccessToken);
          }
        },error:(err)=>{
          this.ErrorSnackBar('Error in getting the connection status')
        }
      })


    }
  }

  onfileChange(event:Event):void{
    const input = event.target as HTMLInputElement;
    this.attachedFileValue = input.value;
    if (input.files && input.files.length > 0) {
      this.attachedFile = input.files[0]; // Capture the first selected file
      console.log('Selected file:', this.attachedFile);
    } else {
      this.attachedFile = null; // Clear if no file is selected
    }
  }


  selectedPages:any[]=[];
  selectedpageName:any[]=[];
  pages: Page[] = [

  ]


  getAdminPages(accessToken:string){
    this.apiSocialService.getAdminPages(accessToken).subscribe({
      next:(success)=>{
        console.log(success)
        var data = success.data
        data.forEach((element:any) => {
          console.log(element)
          let page = {
            id:element.orgId,
            name:element.orgName,
            status:false
          }
          this.pages.push(page)
        });
        if(this.pages.length>0){
          this.isadminpageloading.set(false)
          // this.cdr.detectChanges();
        }
      },
      error:(err)=>{this.ErrorSnackBar('Error in getting the admin pages')}
    })
   
  }


  tooglepages():void{
    this.showPages = !this.showPages
    console.log(this.selectedPages)
    console.log(this.selectedpageName);
    
  }

  pageselected(id:any,name:string):void{
   this.pagechecked = !this.pagechecked
   let page = {
      id:id
   }
   let pageName = {
    name:name
   }
   let pageobj = this.selectedPages.find(x=>x.id == id)
   if(pageobj){
     this.selectedPages.splice(this.selectedPages.indexOf(pageobj),1)
     var updatepageobj = this.pages.find(p=>p.id===id);
     if(updatepageobj){
       updatepageobj.status = !updatepageobj.status
     }
   }else{
    this.selectedPages.push(id)
    this.selectedpageName.push(name);
    var updatepageobj = this.pages.find(p=>p.id===id);
    if(updatepageobj){
      updatepageobj.status = !updatepageobj.status
    }
 
   }

}

Schedule(){
  this.showDatetimePicker = true;
  this.IsPostScheduled = true;
  console.log(this.value)
  console.log(this.formattedTime);
  
}
           
CancelSchedule(){
  this.showDatetimePicker = false;
  this.IsPostScheduled = false;
}

SchedulePost(){
  //convert the datetime to ISO
  const dateString = this.value;  
  const dateObj = new Date(dateString);
  dateObj.setMinutes(dateObj.getMinutes() + 5);
  const isoDateUTC = dateObj.toISOString();
  const isoDateLocal = dateObj.toLocaleString("sv-SE", { timeZoneName: "short" }).replace(" ", "T");
 // convert to IST TO UTC
 let localTime = isoDateLocal.split(" GMT")[0];
// const localTime = "2025-01-15T01:35:09"
let [datePart, timePart] = localTime.split("T");
let [hour, minute, second] = timePart.split(":");
hour = hour.padStart(2, "0");
localTime = `${datePart}T${hour}:${minute}:${second}`;
 const localDate = new Date(localTime);
 const utcDate = new Date(localDate.toISOString());
 const UTCdateObj = new Date(utcDate.toUTCString())
 const UTCDate = UTCdateObj.toISOString();
 console.log(UTCDate);
 var userAccountId = this.userAccountId;
 var text = this.posttextValue;
 var accessToken = this.UserSelectedroviderAccessToken;
 var Pages = this.selectedPages;
 var PageNames = this.selectedpageName;
 //get the users timezone
 console.log(userAccountId)
 console.log(text);
 console.log(accessToken);
 console.log(Pages);
 console.log(PageNames);
 var ianaTimezone = Intl.DateTimeFormat().resolvedOptions().timeZone;
 const now = DateTime.now().setZone(ianaTimezone);
 var userStandardTimezone = now.offsetNameLong;
 this.apiSocialService.ScheduleTextonlyPost(UTCDate,userAccountId,text,accessToken,
  Pages,userStandardTimezone,PageNames).subscribe({
    next:(success)=>{
    if(success.statusCode==200){
      this.SuccessSnackBar('Post has been Scheduled Successfully!')
      this.notificationSound.playSuccess();
      this.selectedOption = 'Twitter';
      this.posttextValue = "";
      this.selectedPages = [];
      this.cdt.detectChanges();
    }
    if(success.statusCode==404){
      this.ErrorSnackBar('Please choose a future date and time to schedule.')
      this.notificationSound.playError();
    }
    },error:(error)=>{
      this.ErrorSnackBar('Post not Scheduled.')
      this.notificationSound.playError();
    }
  })
}

makedirectPost(){
  if(this.attachedFile!=null && this.posttextValue!=null && this.selectedPages.length>0 && this.userName!=null){
    this.apiSocialService.directPostImage(this.attachedFile,this.userAccountId,this.UserSelectedroviderAccessToken,this.selectedPages,this.posttextValue).subscribe({
      next:(success)=>{
        if(success.statusCode==201){
          this.selectedOption = 'Twitter';
          this.posttextValue = "";
          this.selectedPages = [];
          this.attachedFile = null;
          this.attachedFileValue = "";
          this.cdt.detectChanges();
          this.SuccessSnackBar('Post created successfully')
          this.notificationSound.playSuccess();
        }
      },
      error:(err)=>{
        this.ErrorSnackBar('Error in creating the post')
        this.notificationSound.playError();
      }
    })
     
  }else{
    if(this.posttextValue!=null && this.selectedPages.length>0 && this.userName!=null){
      this.apiSocialService.textonlyPost(this.userAccountId,this.posttextValue,this.UserSelectedroviderAccessToken,this.selectedPages).subscribe({
        next:(success)=>{
          if(success.statusCode==200){
            this.selectedOption = 'Twitter';
            this.posttextValue = "";
            this.selectedPages = [];
            this.cdt.detectChanges();
            this.SuccessSnackBar('Post created successfully')
            this.notificationSound.playSuccess();
          }
        },
        error:(err)=>{
          this.ErrorSnackBar('Error in creating the post')
          this.notificationSound.playError();
        }
      })
    }
  }

}

}
