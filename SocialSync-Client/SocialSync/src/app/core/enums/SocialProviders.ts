import { Injectable } from "@angular/core";


export const Providers ={
    Facebook : 'Facebook',
    Instagram : 'Instagram',
    LinkedIn : 'LinkedIn'
} as const;

export type UserRoleType = (typeof Providers)[keyof typeof Providers];