import { Component } from '@angular/core';
import { UntypedFormGroup, UntypedFormControl,Validators, FormBuilder,FormGroup,FormControl, AbstractControl, ValidationErrors } from '@angular/forms';
import { ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Route, Router } from '@angular/router';
import { ApiAccountService } from '../../../../core/services/api.services/api.account.service';
import { MatSnackBar } from '@angular/material/snack-bar';

@Component({
  selector: 'app-resetpassoword',
  standalone: true,
  imports: [ReactiveFormsModule,CommonModule],
  templateUrl: './resetpassoword.component.html',
  styleUrl: './resetpassoword.component.scss'
})
export class ResetpassowordComponent {
 
  resetpasswordForm:UntypedFormGroup;
  userEmail = "";
  loading:boolean=false;

  constructor(private formBuilder:FormBuilder,private activatedRoute:ActivatedRoute,
    private apiServices:ApiAccountService,private snackBar: MatSnackBar,private router:Router){
    this.resetpasswordForm = this.formBuilder.group({ 
      newpassword: ['', [Validators.required, Validators.minLength(6),this.passwordStrengthValidator]],
      confirmPassword:['',[Validators.required]]
    },{validators:this.passwordMatchValidator})
     
    this.activatedRoute.queryParams.subscribe(params=>{
      this.userEmail = params['Email']
    })
    
  }


  passwordStrengthValidator(control: AbstractControl): { [key: string]: boolean } | null {
    const value = control.value;
    if (!value) {
      return null;
    }
    const hasMinLength = value.length >= 6;
    const hasUpperCase = /[A-Z]/.test(value);
    const hasNumber = /[0-9]/.test(value);
    const hasSpecialChar = /[!@#$%^&*(),.?":{}|<>]/.test(value);
  
    const passwordValid = hasMinLength && hasUpperCase && hasNumber && hasSpecialChar;

    return passwordValid ? null: { passwordStrength: true };
  }


  passwordMatchValidator(control: AbstractControl): ValidationErrors | null  {
    const password = control.get('newpassword')?.value;
    const confirmPassword = control.get('confirmPassword')?.value;

    if (password !== confirmPassword) {
      control.get('confirmPassword')?.setErrors({ passwordMismatch: true });
    }
    return null;
  }

  ErrorSnackBar(message: string) {
    this.snackBar.open(message, '❌', {
      verticalPosition: 'top',
      horizontalPosition: 'center',
    });
  }

  SuccessSnackBar(message: string) {
    this.snackBar.open(message, 'close', {
      duration: 3000, 
      verticalPosition: 'top',
      horizontalPosition: 'center',
    });
  }

  resetPassword(){
   if(this.resetpasswordForm.valid){
     let Email = this.userEmail;
     let newPassword = this.resetpasswordForm.value['newpassword'];
     this.loading=true
     this.apiServices.resetPassword(Email,newPassword).subscribe({
      next:(response)=>{
       switch(response.statusCode){
        case 200:
          this.loading=false
          this.SuccessSnackBar('Password reseted Successfully!')
          this.router.navigate(['login'])
          break;
        case 400:
          this.loading=false
          this.ErrorSnackBar('Oops Password was not Reseted!')
          break;
        case 404:
          this.loading=false
          this.ErrorSnackBar('Invalid User or Not verified the Email')
          break
        default:
          this.loading=false
          this.ErrorSnackBar('Oops something went wrong.')
       }

      },error:()=>{
        this.loading=false
        this.ErrorSnackBar('Oops something went wrong.')
      }
     })

   }
  }
}
