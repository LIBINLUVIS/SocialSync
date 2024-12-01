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
}

