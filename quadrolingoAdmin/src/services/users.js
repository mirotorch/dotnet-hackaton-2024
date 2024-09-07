import axios from 'axios';
import { fromBackend } from '../mappers/userMapper';

const baseUrl = 'http://localhost:5214/users'; // Updated URL for fetching languages

let token = null;

const setToken = (newToken) => {
  token = `Bearer ${newToken}`;
};

const getUsersByName = async (name) => {
  const config = {
    headers: { Authorization: token },
  };

  try {
    const response = await axios.get(baseUrl, {
      params: { name } // Passing the name as a query parameter
    }, config);
    console.log('response', response.data)
    return response.data.map(fromBackend)
  } catch (error) {
    console.error('Error fetching users:', error);
    throw error;
  }
};

// Fetch user profile by ID
const getUserProfileById = async (id) => {
  const config = {
    headers: { Authorization: token },
  };

  try {
    const response = await axios.get(`${baseUrl}/${id}/profile`, config);
    console.log('User profile response:', response.data);
    return fromBackend(response.data);
  } catch (error) {
    console.error('Error fetching user profile:', error);
    throw error;
  }
};

const getUserKnownWords = async (id) => {
  const config = {
    headers: { Authorization: token },
  };

  try {
    const response = await axios.get(`${baseUrl}/${id}/known_words`, config);
    console.log('User known words response:', response.data);
    return response.data;
  } catch (error) {
    console.error('Error fetching user known words:', error);
    throw error;
  }
}

const getProgress = async (id) => {
  const config = {
    headers: { Authorization: token },
  };

  try {
    const response = await axios.get(`${baseUrl}/${id}/progress`, config);
    console.log('User progress response:', response.data);
    return response.data;
  } catch (error) {
    console.error('Error fetching user progress:', error);
    throw error;
  }
}


export default { setToken, getUsersByName, getUserProfileById, getUserKnownWords, getProgress };