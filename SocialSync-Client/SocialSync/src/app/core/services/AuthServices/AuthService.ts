import { Injectable, signal, computed } from '@angular/core';

@Injectable({
    providedIn: 'root'
})

export class AuthService{

    private authToken = signal<string | null>(null);

    isAuthenticated = computed(()=>this.authToken()!==null)

    constructor(){
        this.loadTokenFromSession();
    }

    setToken(token:string){
        sessionStorage.setItem('authToken', token);
        this.authToken.set(token);
    }

    getToken(): string | null{
        return this.authToken();
    }

    clearToken():void{
        this.authToken.set(null);
    }

    public loadTokenFromSession(): void {
        const token = sessionStorage.getItem('authToken');
        if (token) {
          this.authToken.set(token);
        }
    }

    public getUserProfile(): string | null{
      var user_token = this.authToken();
      var res = this.parseToken(user_token);

      return res;

    }

    private parseToken(token:string | null){
        const parts = token?.split('.');
        if(parts?.length!==3){
            return null;
        }

        const payload = parts[1];
        const decodedPayload = atob(payload.replace(/-/g, '+').replace(/_/g, '/'));

        const payloadJson = JSON.parse(decodedPayload);

        if (!payloadJson.unique_name) {
            return null;
          }
        return payloadJson.unique_name;
    }
}

