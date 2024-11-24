import { Inject, Injectable } from "@angular/core";
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import environment from "../../environments/environment.dev";    
import { Observable } from "rxjs";

@Injectable({
    providedIn: 'root'
}) 

export class ApiMethods{
    private baseUrl = environment.baseUrl;

    private defaultHeaders = new HttpHeaders({
        'Content-Type': 'application/json',
    });

    constructor(private http:HttpClient){}

    get<T>(
        endpoint: string,
        params?: { key:string,value:string }[],
        customHeaders?: HttpHeaders
      ): Observable<T> {
        let httpParams = new HttpParams();
        if (params) {
          params.forEach(param => {
              httpParams = httpParams.set(param.key, param.value);
          });
      }
        const url = `${this.baseUrl}${endpoint}`;
        const options = {
          headers: customHeaders || this.defaultHeaders,
          params: params ? httpParams  : undefined
        };
    
        return this.http.get<T>(url, options);
      }
      
    post<T>(endpoint:string,body:any,params?:{key:string,value:string}[],
        customHeaders?:HttpHeaders):Observable<T>{
          let httpParams = new HttpParams();
          if (params) {
            params.forEach(param => {
                httpParams = httpParams.set(param.key, param.value);
            });
        }
            const url = `${this.baseUrl}${endpoint}`;
            const options ={
              Headers: customHeaders || this.defaultHeaders,
              params:params ? httpParams :undefined
            }
            return this.http.post<T>(url,body,options);
     }

   

}