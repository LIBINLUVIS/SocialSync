import { Observable } from "rxjs";
import { ApiMethods } from "./api.methods";
import { Injectable } from '@angular/core';

@Injectable({
    providedIn: 'root', 
  })

export class ApiAccountService{
    
    constructor(private apiClient:ApiMethods){}

    signUp(body:any):Observable<any>{
        return this.apiClient.post("/register",body);
    }
    
    signIn(body:any):Observable<any>{
       return this.apiClient.post("/login",body);
    }


}
