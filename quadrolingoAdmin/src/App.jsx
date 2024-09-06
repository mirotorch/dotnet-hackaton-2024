import { useState } from 'react'
import reactLogo from './assets/react.svg'
import viteLogo from '/vite.svg'
import './App.css'

import { useState, useEffect, useRef } from 'react'
import Blog from './components/Blog'
import Notification from './components/Notification'
import Toggleable from './components/Toggleable'
import BlogForm from './components/BlogForm'
import blogService from './services/blogs'
import loginService from './services/login'

const App = () => {
  // login
  const [user, setUser] = useState(null)
  const [username, setUsername] = useState('')
  const [password, setPassword] = useState('')

  // blogs
  const [blogs, setBlogs] = useState([])

  // notification
  const [notification, setNotification] = useState({ message: '' })

  // ref
  const blogFormRef = useRef()


  const handleLogin = async (event) => {
    console.log('logging in with', username, password)
    event.preventDefault()
    try {
      const user = await loginService.login({
        username, password,
      })

      // set notification
      setNotification({
        message: `User ${user.name} logged in`,
      })

      setTimeout(() => {
        setTimeout(() => {
          setNotification({ message: '' })
        }, 2000)
      })

      // save user info
      setUsername('')
      window.localStorage.setItem('loggedBlogappUser', JSON.stringify(user))
      blogService.setToken(user.token)
      setUser(user)

      // set initial fields
      setPassword('')
      setUsername('')
    } catch (error) {
      console.error('Wrong credentials')
      setNotification({
        message: 'wrong username or password',
        isError: true
      })

      setTimeout(() => {
        setTimeout(() => {
          setNotification({ message: '' })
        }, 2000)
      })
    }
  }

  const handleLogout = () => {
    console.log('logging out')
    setNotification({
      message: `User ${user.name} logged out`,
    })

    setTimeout(() => {
      setTimeout(() => {
        setNotification({ message: '' })
      }, 2000)
    })
    window.localStorage.removeItem('loggedBlogappUser')
    setUser(null)
  }

  const handleCreateNewBlog = async (newBlog) => {
    console.log('creating new blog', newBlog)
    try {
      const createdBlog = await blogService.create(newBlog)

      // set notification
      console.log('created blog', createdBlog)
      setNotification({
        message: `a new blog ${newBlog.title} by ${newBlog.author} added`,
      })

      setTimeout(() => {
        setTimeout(() => {
          setNotification({ message: '' })
        }, 2000)
      })

      blogFormRef.current.toggleVisibility()
      setBlogs(blogs.concat(createdBlog))
    } catch (error) {
      // set notification
      console.error('Failed to create new blog', error)
      setNotification({
        message: `failed to create a blog, ${error.response.data.error}`,
        isError: true
      })

      setTimeout(() => {
        setTimeout(() => {
          setNotification({ message: '' })
        }, 2000)
      })
    }
  }

  const updateBlog = async (blog) => {
    try {
      const result = await blogService.update(blog)
      console.log('updated blog', result)
      setBlogs(blogs.map(b => b.id === blog.id ? result : b))
    } catch (error) {
      console.error('Failed to like blog', error)
      setNotification({
        message: `failed to like blog, ${error.response.data.error}`,
        isError: true
      })

      setTimeout(() => {
        setTimeout(() => {
          setNotification({ message: '' })
        }, 2000)
      })
    }
  }

  const deleteBlog = async (blog) => {
    if (!window.confirm(`Remove ${blog.title} by ${blog.author}?`)) {
      return
    }

    try {
      const result = await blogService.remove(blog.id)
      console.log(result)
      setBlogs(blogs.filter(b => b.id !== blog.id))

      setNotification({
        message: `Removed ${blog.title} by ${blog.author}`,
      })

      setTimeout(() => {
        setTimeout(() => {
          setNotification({ message: '' })
        }, 2000)
      })
    } catch (error) {
      console.error('Failed to delete blog', error)
      setNotification({
        message: `failed to delete blog, ${error.response.data.error}`,
        isError: true
      })

      setTimeout(() => {
        setTimeout(() => {
          setNotification({ message: '' })
        }, 2000)
      })
    }
  }

  useEffect(() => {
    const loggedUserJSON = window.localStorage.getItem('loggedBlogappUser')
    if (loggedUserJSON) {
      const user = JSON.parse(loggedUserJSON)
      console.log('retrieving user from local storage', user)
      setUser(user)
      blogService.setToken(user.token)
    }
  }, [])

  useEffect(() => {
    blogService.getAll().then(blogs => {
      setBlogs( blogs )
    })
  }, [])

  if (user === null) {
    return (
      <div>
        <h2>Log in to application</h2>
        <Notification notification={notification} />
        <form onSubmit={handleLogin}>
          <div>
            username
            <input
              type="text"
              value={username}
              name="Username"
              data-testid="username"
              onChange={({ target }) => setUsername(target.value)}
            />
          </div>
          <div>
            password
            <input
              type="password"
              value={password}
              name="Password"
              data-testid="password"
              onChange={({ target }) => setPassword(target.value)}
            />
          </div>
          <button type="submit">login</button>
        </form>
      </div>
    )
  }

  return (
    <div>
      <h2>blogs</h2>
      <Notification notification={notification} />
      <p>{user.name} logged-in<button onClick={handleLogout}>logout</button></p>
      <Toggleable buttonLabel='new blog' ref={blogFormRef}>
        <BlogForm handleCreateNewBlog={handleCreateNewBlog} />
      </Toggleable>
      {blogs.toSorted((a, b) => b.likes - a.likes).map(blog =>
        <Blog key={blog.id} blog={blog} updateBlog={updateBlog} deleteBlog={deleteBlog} username={user.username}/>
      )}
    </div>
  )
}

export default App

export default App
