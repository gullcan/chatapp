import React, { useState } from 'react';
import { SafeAreaView, View, Text, TextInput, Button, StyleSheet, FlatList } from 'react-native';
import { sendMessage } from './api';

export default function App() {
  const [text, setText] = useState('');
  const [items, setItems] = useState([]);

  const onSend = async () => {
    if (!text.trim()) return;
    try {
      const res = await sendMessage(text, 'mobile');
      setItems((prev) => [...prev, { you: text, sentiment: res?.sentiment || 'neutral', id: String(prev.length + 1) }]);
      setText('');
    } catch (e) {
      setItems((prev) => [...prev, { you: text, sentiment: 'error', id: String(prev.length + 1) }]);
      setText('');
    }
  };

  const renderItem = ({ item }) => (
    <View style={styles.item}>
      <Text style={styles.you}>You: {item.you}</Text>
      <Text style={styles.sent}>Sentiment: {item.sentiment}</Text>
    </View>
  );

  return (
    <SafeAreaView style={styles.container}>
      <Text style={styles.title}>Sentiment AI Chat (Mobile)</Text>
      <FlatList
        data={items}
        keyExtractor={(it) => it.id}
        renderItem={renderItem}
        contentContainerStyle={styles.list}
      />
      <View style={styles.inputRow}>
        <TextInput
          style={styles.input}
          placeholder="Type your message..."
          value={text}
          onChangeText={setText}
        />
        <Button title="Send" onPress={onSend} />
      </View>
    </SafeAreaView>
  );
}

const styles = StyleSheet.create({
  container: { flex: 1, backgroundColor: '#fff' },
  title: { fontSize: 20, fontWeight: '600', margin: 16 },
  list: { paddingHorizontal: 16 },
  item: { paddingVertical: 12, borderBottomWidth: StyleSheet.hairlineWidth, borderColor: '#ddd' },
  you: { fontSize: 16, marginBottom: 4 },
  sent: { fontSize: 14, color: '#555' },
  inputRow: { flexDirection: 'row', padding: 16, gap: 8, alignItems: 'center' },
  input: { flex: 1, borderWidth: 1, borderColor: '#ccc', borderRadius: 8, paddingHorizontal: 12, height: 40 },
});
