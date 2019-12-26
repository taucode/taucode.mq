; PUB
(defblock :name pub :is-top t
	(term :value "PUB" :name pub)
	(any-term :name send-to)
	(any-string :name message-text)
	(end)
)
