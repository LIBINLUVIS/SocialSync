import { CommonModule } from '@angular/common';
import { Component, OnInit, signal } from '@angular/core';
import { UntypedFormGroup, UntypedFormControl,Validators, FormBuilder,FormGroup,FormControl } from '@angular/forms';
import { ReactiveFormsModule } from '@angular/forms';
import { OtpTimerComponent } from '../../../../components/otp-timer/OTPTimer/otp-timer.component';
import { ApiAccountService } from '../../../../core/services/api.services/api.account.service';
import { MatSnackBar } from '@angular/material/snack-bar';




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

  constructor(private formBuilder:FormBuilder,private accountService:ApiAccountService,private snackBar: MatSnackBar){
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
      verticalPosition: 'top',
      horizontalPosition: 'center',
    });
  }

  TimerEvent(message:boolean){
    if(message){
      this.IsresendCode.set(true);
    }
  }

  sendemailVerification(){
    if(this.forgotpasswordForm.valid){
      this.formData.set(this.forgotpasswordForm.value)
      this.forgotpasswordForm.get('Email')?.disable();
      this.accountService.forgotPassword(this.formData()).subscribe(
        success =>{
          console.log(success);
          if(success.statusCode == 200){
            this.SuccessSnackBar('please check your Email.')
            this.forgotpasswordForm.reset();
            this.isverificationCode = true
            this.isSubmitting = true
          }
        },
        error=>{
          console.log(error);
        }
      )
      // this.isverificationCode = true;
      // this.isSubmitting = true;
      // this.forgotpasswordForm.get('Email')?.disable();
      
    }
  }

  verifyingCode(){
     if(this.verifycodeForm.valid){
      window.location.href = '/resetpassword';
     }
  }





}
