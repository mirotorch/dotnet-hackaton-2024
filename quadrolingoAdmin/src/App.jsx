import { useState, useEffect } from 'react'

import Word from './components/Word'
import wordsService from './services/words'

const App = () => {
  const [words, setWords] = useState([])
  const [wordLang, setWordLang] = useState('')
  const [newWord, setNewWord] = useState('')

  useEffect(() => {
    wordsService.getAll().then(initialWords => {
      setWords(initialWords);
    });
  }, []);

  const updateWord = async (newWord) => {
    console.log('word updated', newWord);
  }

  const deleteWord = async (newWord) => {
    console.log('word deleted', newWord);
  }

  const handleAddWord = async (newWord) => {
    console.log('created word', newWord)

  }

  return (
    <div>
      <h2>words</h2>
      {words.map(w =>
        <Word key={w.id} word={w.base} wordLang={w.lang} updateWord={updateWord} deleteWord={deleteWord}/>
      )}

      <form onSubmit={handleAddWord}>
      <input
        type="text"
        value={newWord}
        onChange={(e) => setNewWord(e.target.value)}
        placeholder="Add a new word"
      />
      <button type="submit">Add Word</button>
      </form>
    </div>
  )
}

export default App
