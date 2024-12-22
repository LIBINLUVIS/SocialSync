import { Component } from '@angular/core';
import { ApiAccountService } from '../../../core/services/api.services/api.account.service';
import { ApiSocialService } from '../../../core/services/api.services/api.social.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import ApistorageService from '../../../core/services/localstorageService/api,storage.service';
import { CommonModule } from '@angular/common';


@Component({
  selector: 'app-account-connect',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './account-connect.component.html',
  styleUrl: './account-connect.component.scss'
})
export class AccountConnectComponent {


   userId:string= ""
   accountuserId:number = 0;
   LinkedinConnStatus:string = "Disconnected"

  constructor(private apiService:ApiAccountService,
    private socialapiService:ApiSocialService, private snackBar: MatSnackBar,private locastorageService:ApistorageService){}

  ngOnInit(){
  
      var userid = sessionStorage.getItem('Userid')
      if(userid!=null)
        this.userId = userid
      // calling the api to get the user id
      if(this.userId!=null){
        console.log(this.userId)
        this.apiService.getuserId(this.userId).subscribe({
          next:(success)=>{
            if(success.statusCode==200){
              this.accountuserId = success.data
              console.log(this.accountuserId);
            }
          },
          error:(err)=>{
           this.accountuserId = 0;
           this.ErrorSnackBar('Error in getting the user Account Id');
          }
        })
      }
      //checking the conn status
      if(userid!=null){
        this.apiService.getConnectionStatus(userid).subscribe({
          next:(success)=>{
          
            if(success[0].Provider =="Linkedin" && success[0].IsActive == 'true'){
               this.LinkedinConnStatus = "Connected"
            }else{
              this.LinkedinConnStatus = "Disconnected"
            }
          },error:(err)=>{
           
          }
        })
      }
  }


  ErrorSnackBar(message: string) {
    this.snackBar.open(message, '❌', {
      verticalPosition: 'top',
      horizontalPosition: 'center',
    });
  }

  SuccessSnackBar(message: string) {
    this.snackBar.open(message, 'close', {
      duration: 2000, 
      verticalPosition: 'top',
      horizontalPosition: 'center',
    });
  }

  ConnectLinkedin(){
    const clientId = "863yknun9tywlo";
    const redirectUrl = "http://localhost:4200/accounts";
    const state = Math.random().toString(36).substring(7);
    const scope = "openid,profile,email";

    const authUrl = `https://www.linkedin.com/oauth/v2/authorization?response_type=code&client_id=${clientId}&redirect_uri=${encodeURIComponent(
      redirectUrl
    )}&state=${state}&scope=${encodeURIComponent(scope)}`;

    //open the window for auth

    const width = 600;
    const height = 500;
    const left = window.screen.width / 2 - width /2;
    const top = window.screen.height / 2 - height/2;

    const popup = window.open(
      authUrl,
      'LinkedInAuth',
      `width=${width},height=${height},top=${top},left=${left}`
    );

        // Poll the popup to check if it redirected
    const interval = setInterval(() => {
        if (!popup || popup.closed) {
            clearInterval(interval);
            return;
        }

      try{

        const popupUrl = new URL(popup.location.href);
        if (popupUrl.origin === window.location.origin) {
          const code = popupUrl.searchParams.get('code');
          const stateReceived = popupUrl.searchParams.get('state');
          clearInterval(interval);
          if (stateReceived === state) {
            if(code!=null && this.accountuserId!=0)
              this.socialapiService.getLinkedinAccessToken(code,this.accountuserId).subscribe({
               next:(success)=>{
                 if(success.statusCode==200){
                  popup.close();
                  this.SuccessSnackBar("Successfully connected!");
                  // keep the accesstoken in the localstorage and close the auth window
                  var Ltoken = success.data
                  this.locastorageService.set("LinkedinAccesstoken",Ltoken)
                  this.LinkedinConnStatus = "Connected";
                 }
               },
               error:(err)=>{
                popup.close();

                // make a toast showing the error
                this.ErrorSnackBar("oops something went wrong while connecting")
                // console.log("oops something went wrong",err)
               }
            })
          } else {
            console.error('State mismatch!');
          }
          // popup.close();
          // clearInterval(interval);
        }

      }catch{
        //  this.ErrorSnackBar("oops something went wrong while connecting")
      }

    },500);
  }

  DisconnectLinkedin(){
   //api for disconnecting the social account
   this.apiService.disconnectSocialAccount(this.accountuserId,"Linkedin").subscribe({
    next:(response)=>{
      this.LinkedinConnStatus = "Disconnected";
      
    },error:(err)=>{
      this.LinkedinConnStatus = "Connected"
      this.ErrorSnackBar('Something went wrong while disconnecting!')
    }
   })
  }

}
