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

    test():Observable<any>{
        return this.apiClient.get("Test");
    }


}
