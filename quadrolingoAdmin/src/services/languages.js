import axios from 'axios';
const baseUrl = 'http://localhost:3001/languages'; // Updated URL for fetching languages

let token = null;

const setToken = (newToken) => {
  token = `Bearer ${newToken}`;
};

// Fetch all languages from the API
const getAll = async () => {
  try {
    const response = await axios.get(baseUrl);
    return response.data;
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
    const response = await axios.post(baseUrl, newObject, config);
    return response.data;
  } catch (error) {
    console.error('Error creating a new language:', error); 
    throw error;
  }
};

// Update an existing language entry
const update = async (newObject) => {
  const config = {
    headers: { Authorization: token },
  };
  try {
    const response = await axios.put(`${baseUrl}/${newObject.id}`, newObject, config);
    return response.data;
  } catch (error) {
    console.error('Error updating the language:', error); 
    throw error;
  }
};

// Delete a language entry by ID
const remove = async (id) => {
  const config = {
    headers: { Authorization: token },
  };
  try {
    const response = await axios.delete(`${baseUrl}/${id}`, config);
    return response.data;
  } catch (error) {
    console.error('Error deleting the language:', error); 
    throw error;
  }
};

export default { setToken, getAll, create, update, remove };