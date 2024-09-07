const Word = ({ word, wordLang, updateWord, deleteWord }) => {
  return (
    <div>
      <p>
        {word} ({wordLang})
        <button onClick={() => updateWord(word)}>Update Word</button>
        <button onClick={() => deleteWord(word)}>Delete Word</button>
      </p>      
    </div>
  );
};

export default Word