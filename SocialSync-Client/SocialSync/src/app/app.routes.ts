import { Routes } from '@angular/router';

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
    redirectTo: 'login',
    pathMatch: 'full'
  }

];
