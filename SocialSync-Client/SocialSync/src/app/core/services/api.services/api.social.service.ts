import { Observable } from "rxjs";
import { ApiMethods } from "./api.methods";
import { Injectable } from "@angular/core";
import { HttpHeaders } from "@angular/common/http";

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

    getAdminPages(accesstoken:string):Observable<any>{
      const endpoint = '/Social/getLinkedinAdminPages';
      const params = [
        {key:'accessToken',value:accesstoken}
      ];
      return this.apiClient.get(endpoint,params);
    }
    textonlyPost(userId:string,text:string,AccessToken:string,PageId:Array<string>):Observable<any>{
      const endpoint = '/Social/TextOnlyPost';
      const params = [
        {key:'UserID',value:userId.toString()},
        {key:'text',value:text},
        {key:'AccessToken',value:AccessToken}
    
      ];
    
      return this.apiClient.post(endpoint,PageId,params);
    }

    directPostImage(attachedFile:File,userId:string,AccessToken:string,PageId:Array<string>,text:string):Observable<any>{
      const endpoint = '/Social/PostImage';
      const formData = new FormData();
      formData.append('file',attachedFile);
      formData.append('UserID',userId);
      formData.append('AccessToken',AccessToken);
      formData.append('PostText',text);
      PageId.forEach((pageId)=>{
        formData.append('PageId',pageId);
      });
      return this.apiClient.post(endpoint,formData);
    }





}