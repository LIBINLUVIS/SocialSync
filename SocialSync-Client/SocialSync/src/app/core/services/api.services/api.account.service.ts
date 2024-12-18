import { Observable } from "rxjs";
import { ApiMethods } from "./api.methods";
import { Injectable } from '@angular/core';

@Injectable({
    providedIn: 'root', 
  })

export class ApiAccountService{
    
    constructor(private apiClient:ApiMethods){}

    signUp(body:any):Observable<any>{
        return this.apiClient.post("/Account/userRegister",body);
    }
    
    signIn(body:any):Observable<any>{
       return this.apiClient.post("/Account/login",body);
    }

    forgotPassword(body:any):Observable<any>{
        return this.apiClient.post("/Account/Forgotpassword",body);
    }

    verifyCode(email:string,code:string):Observable<any>{
        const endpoint = '/Account/VerifyCode';
        const params = [
            { key: 'email', value: email },
            { key: 'code', value: code }
        ];
        return this.apiClient.get(endpoint,params);
    }
    resetPassword(email:string,newPassword:string):Observable<any>{
        const endpoint ='/Account/ResetPassword';
        const params = [
            {key:'email',value:email},
            {key:'newPassword',value:newPassword}
        ];
        return this.apiClient.post(endpoint,undefined,params);
    }

    test():Observable<any>{
        return this.apiClient.get("Test");
    }
    getuserId(userId:string):Observable<any>{
        const endpoint = "/Account/GetUserId"
        const params = [
            { key: 'userId', value: userId }
        ];
      return this.apiClient.get(endpoint,params);
    }

    getConnectionStatus(userId:string):Observable<any>{
        const endpoint = '/Account/SocialConnectionStatus'
        const params = [
            {key:'userId',value:userId}
        ]
        return this.apiClient.get(endpoint,params)
    }
    disconnectSocialAccount(userId:number,Provider:string){
        const endpoint = '/Account/DisconnectSocialaccount'
        const params = [
            {key:'userId',value:userId.toString()},
            {key:'Provider',value:Provider}
        ]

        return this.apiClient.post(endpoint,undefined,params);
    }

    // getAuth(authCode:string):Observable<any>{
    //   const endpoint = 'https://localhost:7220/Social/AccessToken';
    //   const params = [
    //     {key:'authCode',value:authCode}
    //   ];
    //   return this.apiClient.getauth(endpoint,params);
    // }


}
