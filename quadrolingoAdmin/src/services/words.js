import axios from 'axios'

import { fromBackend, toBackend } from '../mappers/wordMapper';

const baseUrl = 'http://localhost:5214/words';

let token = null
const setToken = newToken => {
  token = `Bearer ${newToken}`
}

const getAll = async () => {
  try {
    const response = await axios.get(baseUrl)
    return response.data.map(fromBackend)
  } catch (error) {
    console.error('Error fetching words:', error)
    throw error
  }
}

const create = async newObject => {
  const config = {
    headers: { Authorization: token },
  }

  try {
    const response = await axios.post(baseUrl, toBackend(newObject), config)
    console.log('inserted data', response.data)
    return fromBackend(response.data)
  } catch (error) {
    console.error('Error creating a word:', error)
    throw error
  }
}

const update = async newObject => {
  const config = {
    headers: { Authorization: token },
  }
  const url = `${baseUrl}/${newObject.id}`
  console.log('request url:', url)
  try {
    console.log(toBackend(newObject))
    const response = await axios.put(url, toBackend(newObject), config)
    return response.data
  } catch (error) {
    console.error('Error updating a word', error)
    throw error
  }
}

const remove = async id => {
  const config = {
    headers: { Authorization: token },
  }
  const url = `${baseUrl}/${id}`
  console.log('request url:', url)
  const response = await axios.delete(url, config)
  return response.data
}

export default { setToken, getAll, create, update, remove }
