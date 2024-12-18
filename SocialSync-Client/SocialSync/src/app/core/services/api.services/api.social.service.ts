import { Observable } from "rxjs";
import { ApiMethods } from "./api.methods";
import { Injectable } from "@angular/core";

@Injectable({
    providedIn: 'root', 
})

export class ApiSocialService{ 
  
    constructor(private apiClient:ApiMethods){}
    
    getLinkedinAccessToken(authCode:string,accountuserId:number):Observable<any>{
      const endpoint = '/Social/LinkedinAccessToken';
      const params = [
        {key:'authCode',value:authCode},
        {key:'accountUserId',value:accountuserId.toString()}
      ];
      return this.apiClient.get(endpoint,params);
    }





}