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
  {
    path:'accounts',
    loadComponent:()=>import('./Pages/social/account-connect/account-connect.component').then(m=>m.AccountConnectComponent),
    canActivate:[AuthGuard]

  },
  {
    path:'CreatePost',
    loadComponent:()=>import('./Pages/social/create-post-component/create-post-component.component').then(m=>m.CreatePostComponentComponent),
    canActivate:[AuthGuard]
  },
  {
    path:'MyPage',
    loadComponent:()=>import('./Pages/social/mypages/mypages.component').then(m=>m.MypagesComponent),
    canActivate:[AuthGuard]
  },
  {
    path:'MySchedules',
    loadComponent:()=>import('./Pages/social/my-schedulers/my-schedulers.component').then(m=>m.MySchedulersComponent),
    canActivate:[AuthGuard]
  }


];
