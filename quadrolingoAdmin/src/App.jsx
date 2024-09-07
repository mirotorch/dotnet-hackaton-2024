import { useState, useEffect } from 'react'

import Word from './components/Word'
import WordForm from './components/WordForm'

import wordsService from './services/words'
import languagesService from './services/languages'

const App = () => {
  const [words, setWords] = useState([])
  const [languages, setLanguages] = useState([])

  // Fetch words from the API
  useEffect(() => {
    wordsService.getAll().then(initialWords => {
      setWords(initialWords);
    });
  }, []);

  // Fetch languages from the API
  useEffect(() => {
    languagesService.getAll().then(initialLanguages => {
      setLanguages(initialLanguages);
    });
  }, []);

  const updateWord = async (newWord) => {
    console.log('word updated', newWord);
  }

  const deleteWord = async (newWord) => {
    console.log('word deleted', newWord);
  }

  const addWord = async (newWord) => {
    console.log('word added', newWord);
    wordsService.create(newWord).then(returnedWord => {
      setWords(words.concat(returnedWord));
    });
  }

  return (
    <div>
      <h2>Words</h2>
      {words.map(w => (
        <Word key={w.id} word={w.base} wordLang={w.lang} updateWord={updateWord} deleteWord={deleteWord} />
      ))}

      <h2>Add Word</h2>
      <WordForm languages={languages} onSubmit={addWord} />

     </div>
  )
}

export default App
