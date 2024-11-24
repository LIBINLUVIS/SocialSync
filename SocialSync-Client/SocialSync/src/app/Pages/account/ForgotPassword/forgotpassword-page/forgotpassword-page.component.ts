import { CommonModule } from '@angular/common';
import { Component, OnInit, signal } from '@angular/core';
import { UntypedFormGroup, UntypedFormControl,Validators, FormBuilder,FormGroup,FormControl } from '@angular/forms';
import { ReactiveFormsModule } from '@angular/forms';
import { OtpTimerComponent } from '../../../../components/otp-timer/OTPTimer/otp-timer.component';
import { ApiAccountService } from '../../../../core/services/api.services/api.account.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Route, Router } from '@angular/router';




@Component({
  selector: 'app-forgotpassword-page',
  standalone: true,
  imports: [ReactiveFormsModule,CommonModule,OtpTimerComponent],
  templateUrl: './forgotpassword-page.component.html',
  styleUrl: './forgotpassword-page.component.scss'
})
export class ForgotpasswordPageComponent implements OnInit {

  forgotpasswordForm:UntypedFormGroup;
  verifycodeForm:UntypedFormGroup;
  isverificationCode:boolean = false;
  isSubmitting: boolean = false;
  formData = signal<any>(null);
  IsresendCode = signal<boolean>(false);
  timerReset:boolean = false;
  IsverificationbtnDisabled:boolean = false;
  loadingbtn1:boolean = false
  loadingbtn2:boolean = false
  loadingbtn3:boolean = false

  constructor(private formBuilder:FormBuilder,private accountService:ApiAccountService
    ,private snackBar: MatSnackBar,private router:Router){
    this.forgotpasswordForm = this.formBuilder.group({
      Email: ['', [Validators.required, Validators.email]],
    })
    this.verifycodeForm = this.formBuilder.group({
      Code: ['', [Validators.required]],
    })
  }
  ngOnInit():void{
     
  }

  SuccessSnackBar(message: string) {
    this.snackBar.open(message, 'close', {
      duration: 2000, 
      verticalPosition: 'top',
      horizontalPosition: 'center',
    });
  }

  ErrorSnackBar(message: string) {
    this.snackBar.open(message, '❌', {
      verticalPosition: 'top',
      horizontalPosition: 'center',
    });
  }

  TimerEvent(message:boolean){
    if(message){
      this.timerReset = false;
      this.IsresendCode.set(true);
    }
  }

  resetTimer(){
    this.timerReset = true;
    this.IsresendCode.set(false);
    this.loadingbtn2=true
    this.accountService.forgotPassword(this.formData()).subscribe(
      success =>{
        console.log(success);
        if(success.statusCode == 200){
          this.loadingbtn2=false
          this.SuccessSnackBar('OTP resented successfully!')
          this.isverificationCode = true
          this.isSubmitting = true
        }
      },
      error=>{
        this.loadingbtn2=false
        this.ErrorSnackBar('Oops Something went wrong!')
      }
    )
  }

  sendemailVerification(){
    if(this.forgotpasswordForm.valid){
      this.formData.set(this.forgotpasswordForm.value)
      this.forgotpasswordForm.get('Email')?.disable();
      this.loadingbtn1 = true;
      this.accountService.forgotPassword(this.formData()).subscribe(
        success =>{
          console.log(success);
          if(success.statusCode == 200){
            this.SuccessSnackBar('please check your Email.')
            // this.forgotpasswordForm.reset();
            this.loadingbtn1=false
            this.IsverificationbtnDisabled = true;
            this.isverificationCode = true
            this.isSubmitting = true
          }
          if(success.statusCode == 404){
            this.loadingbtn1=false
            this.ErrorSnackBar('User is not registered with this Email.')
          }
        },
        error=>{
          this.loadingbtn1=false
          this.ErrorSnackBar('Oops Something went wrong!')
        }
      )
      // this.isverificationCode = true;
      // this.isSubmitting = true;
      // this.forgotpasswordForm.get('Email')?.disable();
      
    }
  }

  verifyingCode(){
     if(this.verifycodeForm.valid){
     let code = this.verifycodeForm.value?.Code;
     let email = this.forgotpasswordForm.value?.Email;
     this.loadingbtn3=true
     this.accountService.verifyCode(email
      ,code).subscribe(
     {
      next:(response)=>{
        const statusCode = Number(response.statusCode);
        console.log(statusCode)
         switch(statusCode){
          case 200:
            this.loadingbtn3=false
            this.SuccessSnackBar('OTP Verified!')
            const queryParams ={
              Email:email
            }
            this.router.navigate(['resetpassword'],{
              queryParams:queryParams
            })
            break
          case 404:
            this.loadingbtn3=false
            this.ErrorSnackBar('Incorrect OTP')
            break
          case 401:
            this.loadingbtn3=false
            this.ErrorSnackBar('OTP Expired,Resend OTP')
            break
          default:
            this.loadingbtn3=false
            this.ErrorSnackBar('Oops Something went wrong!')

         }

      },error:(err)=> {
        this.loadingbtn3=false
        this.ErrorSnackBar('Oops Something went wrong!')
      },
     }
     )
      // window.location.href = '/resetpassword';
     }
  }





}
