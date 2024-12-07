import React from 'react';
import BlogList from './BlogList'; // Ensure this path is correct

const Home = () => {
    return (
        <div>
            <header>
                <h1>Welcome to My Blog</h1>
                <p>Your go-to source for insightful articles and updates.</p>
            </header>
            <main>
                <BlogList />
            </main>
        </div>
    );
};

export default Home;