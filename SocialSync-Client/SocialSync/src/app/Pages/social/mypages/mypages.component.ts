import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { BrowserModule } from '@angular/platform-browser';

@Component({
  selector: 'app-mypages',
  standalone: true,
  imports: [FormsModule,CommonModule], 
  templateUrl: './mypages.component.html',
  styleUrl: './mypages.component.scss'
})
export class MypagesComponent {
  socialProviders: string[] = ['Facebook', 'Instagram', 'LinkedIn'];
  selectedProvider: string | null = null;

  pages: { id: number; name: string }[] = [];
  selectedPage: number | null = null;

  posts: { title: string; analytics: { likes: number; views: number; impressions: number } }[] = [];

  fetchPages(): void {
    if (this.selectedProvider) {
      // Replace with actual API call
      this.pages = [
        { id: 1, name: `${this.selectedProvider} Page 1` },
        { id: 2, name: `${this.selectedProvider} Page 2` },
      ];
    }
  }

  fetchPosts(): void {
    if (this.selectedPage !== null) {
      // Replace with actual API call
      this.posts = [
        { title: 'Post 1', analytics: { likes: 100, views: 500, impressions: 1200 } },
        { title: 'Post 2', analytics: { likes: 50, views: 300, impressions: 800 } },
      ];
    }
  }



}
