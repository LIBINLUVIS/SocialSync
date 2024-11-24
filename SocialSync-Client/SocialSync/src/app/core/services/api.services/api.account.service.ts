import { Observable } from "rxjs";
import { ApiMethods } from "./api.methods";
import { Injectable } from '@angular/core';

@Injectable({
    providedIn: 'root', 
  })

export class ApiAccountService{
    
    constructor(private apiClient:ApiMethods){}

    signUp(body:any):Observable<any>{
        return this.apiClient.post("userRegister",body);
    }
    
    signIn(body:any):Observable<any>{
       return this.apiClient.post("login",body);
    }

    forgotPassword(body:any):Observable<any>{
        return this.apiClient.post("Forgotpassword",body);
    }

    verifyCode(email:string,code:string):Observable<any>{
        const endpoint = 'VerifyCode';
        const params = [
            { key: 'email', value: email },
            { key: 'code', value: code }
        ];
        return this.apiClient.get(endpoint,params);
    }
    resetPassword(email:string,newPassword:string):Observable<any>{
        const endpoint ='ResetPassword';
        const params = [
            {key:'email',value:email},
            {key:'newPassword',value:newPassword}
        ];
        return this.apiClient.post(endpoint,undefined,params);
    }

    test():Observable<any>{
        return this.apiClient.get("Test");
    }


}
