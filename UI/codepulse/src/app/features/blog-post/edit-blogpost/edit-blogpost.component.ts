import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Observable, Subscription } from 'rxjs';
import { BlogPostService } from '../services/blog-post.service';
import { BlogPost } from '../models/blog-post.model';
import { CategoryService } from '../../category/services/category.service';
import { Category } from '../../category/models/category.model';
import { UpdateBlogPost } from '../models/update-blog-post.model';

@Component({
  selector: 'app-edit-blogpost',
  templateUrl: './edit-blogpost.component.html',
  styleUrls: ['./edit-blogpost.component.css']
})
export class EditBlogpostComponent implements OnInit, OnDestroy {

  id: string | null = null;
  routeSubsciption?: Subscription;
  updateBlogPostSubsciption?: Subscription;
  getBlogPostSubsciption?: Subscription;
  model?: BlogPost;
  categories$?: Observable<Category[]>;
  selectedCategories?: string[];

  constructor(private route: ActivatedRoute,
    private blogPostService: BlogPostService,
    private categoryService: CategoryService,
    private router: Router) {

  }

  onFormSubmit(): void {
    // convert this model to request obj
    if(this.model && this.id){
      var updateBlogPost: UpdateBlogPost = {
        author: this.model.author,
        content: this.model.content,
        shortDescription: this.model.shortDescription,
        featuredImageUrl: this.model.featuredImageUrl,
        isVisible: this.model.isVisible,
        publishedDate: this.model.publishedDate,
        title: this.model.title,
        urlHandle: this.model.urlHandle,
        categories: this.selectedCategories ?? []
      };

      this.updateBlogPostSubsciption = this.blogPostService.updateBlogPost(this.id, updateBlogPost)
      .subscribe({
        next: (response) => {
          this.router.navigateByUrl('/admin/blogposts');
        }
      })
    }
  }

  ngOnInit(): void {
    this.categories$ = this.categoryService.getAllCategories();

    this.routeSubsciption = this.route.paramMap.subscribe({
      next: (params) => {
        this.id = params.get('id');

        // get blogpost from API
        if (this.id) {
          this.getBlogPostSubsciption = this.blogPostService.getBlogPostById(this.id)
            .subscribe({
              next: (response) => {
                this.model = response;
                this.selectedCategories = response.categories.map(x => x.id);
              }
            })
        }
      }
    })
  }

  ngOnDestroy(): void {
    this.routeSubsciption?.unsubscribe();
    this.updateBlogPostSubsciption?.unsubscribe();
    this.getBlogPostSubsciption?.unsubscribe();
  }

}
