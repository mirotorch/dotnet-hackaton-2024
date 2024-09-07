import axios from 'axios';
import { fromBackend, toBackend } from '../mappers/languageMapper';
const baseUrl = 'http://localhost:5214/languages'; // Updated URL for fetching languages

let token = null;

const setToken = (newToken) => {
  token = `Bearer ${newToken}`;
};

// Fetch all languages from the API
const getAll = async () => {
  try {
    const response = await axios.get(baseUrl);
    return response.data.map(fromBackend);
  } catch (error) {
    console.error('Error fetching languages:', error); 
    throw error;
  }
};

// Create a new language entry in the database
const create = async (newObject) => {
  const config = {
    headers: { Authorization: token },
  };

  try {
    const response = await axios.post(baseUrl, toBackend(newObject), config);
    return fromBackend(response.data);
  } catch (error) {
    console.error('Error creating a new language:', error);
    throw error;
  }
};

export default { setToken, getAll, create };