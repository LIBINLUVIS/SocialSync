import { Routes } from '@angular/router';
import { AuthGuard } from './core/services/AuthServices/auth.guard';

export const routes: Routes = [
  {
    path: 'login',
    loadComponent: () => import('./Pages/account/SignIn/login-page/login-page.component').then(m => m.LoginPageComponent)
  },

  {
    path: 'signup',
    loadComponent: () => import('./Pages/account/SignUp/signup-page/signup-page.component').then(m => m.SignupPageComponent)
  },
  {
    path:'forgotpassword',
    loadComponent: () => import('./Pages/account/ForgotPassword/forgotpassword-page/forgotpassword-page.component').then(m => m.ForgotpasswordPageComponent)
  },
  {
    path:'resetpassword',
    loadComponent:()=>import('./Pages/account/ResetPassword/resetpassoword/resetpassoword.component').then(m=>m.ResetpassowordComponent)
  },

  {
    path: '',
    redirectTo: 'home',
    pathMatch: 'full'
   
  },
  {
    path:'home',
    loadComponent:()=>import('./Pages/home/home/home.component').then(m=>m.HomeComponent),
    canActivate:[AuthGuard]
  },


];
