import React from 'react';
import BlogPost from './BlogPost';

const BlogList = () => {
    const posts = [
        { id: 1, title: "First Blog Post", content: "This is my first blog post!" },
        { id: 2, title: "Second Blog Post", content: "This is my second blog post!" },
    ];

    return (
        <div>
            {posts.map(post => (
                <BlogPost key={post.id} title={post.title} content={post.content} />
            ))}
        </div>
    );
};

export default BlogList;