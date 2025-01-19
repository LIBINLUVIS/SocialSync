import { CommonModule } from '@angular/common';
import { Component, ViewChild } from '@angular/core';
import { FormsModule, isFormArray } from '@angular/forms';
import { BrowserModule } from '@angular/platform-browser';
import { HeaderComponent } from '../../../Layouts/header/header.component';
import { AuthService } from '../../../core/services/AuthServices/AuthService';
import { Providers } from '../../../core/enums/SocialProviders';
import { ApiSocialService } from '../../../core/services/api.services/api.social.service';
import ApistorageService from '../../../core/services/localstorageService/api,storage.service';
import { NgxLoadingModule } from "ngx-loading";
import { MatSnackBar } from '@angular/material/snack-bar';
import Howlsound from '../../../components/notification/Howlsound';
import { ApiAccountService } from '../../../core/services/api.services/api.account.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-mypages',
  standalone: true,
  imports: [FormsModule,CommonModule,HeaderComponent,NgxLoadingModule], 
  templateUrl: './mypages.component.html',
  styleUrl: './mypages.component.scss'
})
export class MypagesComponent {

  @ViewChild('headerComp', { static: false }) headerComp!: HeaderComponent;

  formData = {AccessToken:"",PageId:""}
  socialProviders: string[] = [Providers.Facebook, Providers.Instagram, Providers.LinkedIn];
  selectedProvider: string | null = null;
  userName:string = "";
  pages: { id: number; name: string }[] = [];
  selectedPage: number | null = null;
  isLoading:boolean = true
  posts: { commentary: string; postImageUrl:string; profileName:string; publishedAt:string }[] = [];
  public loading = false;

  constructor(private authService:AuthService,private socialService:ApiSocialService,
    private localstorageService:ApistorageService,private snackBar: MatSnackBar,
    private notificationSound:Howlsound,
  private apiaccountService:ApiAccountService,
private router:Router) {}
  ngOnInit(){
    var userName = this.authService.getUserProfile();
    if(userName){
      this.userName = userName
    }else{
      this.userName = ""
    }
  }


  ErrorSnackBar(message: string) {
    this.snackBar.open(message, '❌', {
      verticalPosition: 'bottom',
      horizontalPosition: 'center',
      panelClass: 'custom-snackbar',
    });
  }

  fetchPages(): void {
    var userid = sessionStorage.getItem('Userid')
    if (this.selectedProvider === 'LinkedIn' && userid!=null) {
      //check the user is having an active accessToken
      this.apiaccountService.getConnectionStatus(userid).subscribe({
        next:(success)=>{
          console.log(success)
          if(success[0].Provider =="Linkedin" && success[0].IsActive == 'false'){
            this.router.navigate(['accounts'])
         }else{
          var accessToken = this.localstorageService.get("LinkedinAccesstoken");
          this.formData.AccessToken = accessToken?.toString() || "";
          if(accessToken!=null){
            this.socialService.getAdminPages(accessToken).subscribe({
              next:(success)=>{
                if(success.statusCode==200){
                  success.data.forEach((page:any)=>{
                    this.pages.push({id:page.orgId,name:page.orgName})
                  })
                  if(this.pages.length>0){
                    this.isLoading = false;
                  }
                }
              },
              error:(err)=>{
                this.ErrorSnackBar('Error in getting the pages');
                this.notificationSound.playError();
              }
            });
          }
         }
        },
        error:(err)=>{
          this.ErrorSnackBar('User Connection Status Check Failed');
          this.notificationSound.playError();
        }
      })
    }
  }

  fetchPosts(): void {
    if (this.selectedPage !== null) {
    this.loading = true;
    this.posts = [
    ];
    this.socialService.getLatestPagePosts(this.formData).subscribe({
      next:(success)=>{
        if(success.statusCode==200){
          success.data.forEach((post:any)=>{
            this.posts.push({commentary:post.commentary,postImageUrl:post.ImageUrl,profileName:post.ProfileName,publishedAt:post.PublishedAt})
          })
          this.loading = false;
        }
        
      },
      error:(err)=>{
        this.ErrorSnackBar('Error while fetching the Posts');
        this.notificationSound.playError();
      }
    })
    }
  }



}
