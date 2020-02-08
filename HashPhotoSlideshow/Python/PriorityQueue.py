from collections import deque
import heapq as hq

class Solution(object):
    def __init__(self, file_path):
        self.file_path = file_path
        self.H_sets = [] # each value in a set represents a tag
        self.V_sets = [] # each value in a set represents a tag
        self.V_pairs = [] # tuples of 2 vertical photos (indexes in V_sets)
        self.tag_index_dict = {}
        self.H_ID_mapping = [] # map H_sets index to photo ID
        self.V_ID_mapping = [] # map V_sets index to photo ID
        self.min_heap = []
        self.max_val = 0
        self.max_list = []
        self.num_H = 0
        self.num_V2 = 0
        self.memo = {}
        self.heapq_id = 0

    def load_data(self):
        f = open(self.file_path, 'r')
        N = int(f.readline())
        photo_ID = 0
        for line in f:
            line = line.strip()
            photo_info = line.split(' ')
            orientation = photo_info[0]
            num_tags = int(photo_info[1])
            tag_list = photo_info[2:]
            self.populate_structures(orientation, num_tags, tag_list, photo_ID)
            photo_ID = photo_ID + 1
        self.num_H = len(self.H_sets)
        self.populate_V_pairs()
        self.num_V2 = len(self.V_pairs)
        self.initialize_heap()
        self.find_slideshow()
        self.print_solution()

    def initialize_heap(self):
        for index, h_set in enumerate(self.H_sets):
            id_set = set()
            id_set.add(self.H_ID_mapping[index])
            obj = heapq_object(deque([index]), id_set)
            hq.heappush(self.min_heap, (0, self.heapq_id, obj))
            self.heapq_id = self.heapq_id + 1
        for index, v_pair in enumerate(self.V_pairs):
            v1, v2 = self.V_pairs[index]
            id_set = set()
            id_set.add(self.V_ID_mapping[v1])
            id_set.add(self.V_ID_mapping[v2])
            obj = heapq_object(deque([index + self.num_H]), id_set)
            hq.heappush(self.min_heap, (0, self.heapq_id, obj))
            self.heapq_id = self.heapq_id + 1

    def populate_V_pairs(self):
        V_num = len(self.V_sets)
        for v1 in range(0, V_num):
            for v2 in range(v1 + 1, V_num):
                self.V_pairs.append((v1, v2))

    def calc_interest_factor(self, tag_set_1, tag_set_2):
        interest_1 = len(tag_set_1.intersection(tag_set_2))
        interest_2 = len(tag_set_1.difference(tag_set_2))
        interest_3 = len(tag_set_2.difference(tag_set_1))
        return min(interest_1, min(interest_2, interest_3))

    def populate_structures(self, orientation, num_tags, tag_list, photo_ID):
        tag_set = set()
        for tag in tag_list:
            if not tag in self.tag_index_dict:
                self.tag_index_dict[tag] = len(self.tag_index_dict)
            tag_set.add(self.tag_index_dict[tag])
        if orientation == 'H':
            self.H_sets.append(tag_set)
            self.H_ID_mapping.append(int(photo_ID))
        if orientation == 'V':
            self.V_sets.append(tag_set)
            self.V_ID_mapping.append(int(photo_ID))               

    def find_slideshow(self):
        num_H = len(self.H_sets)
        num_V2 = len(self.V_pairs)
        num_slides = num_H + num_V2
        while len(self.min_heap) > 0:
            current_val, heapq_id, current_obj = hq.heappop(self.min_heap)
            for i in range(num_slides):
                temp_set = self.index_to_photoIDs(i)
                if len(temp_set.intersection(current_obj.id_set)) > 0:
                    continue
                # calculate interest factor
                current_index = current_obj.index_list[-1]
                key1 = str(i) + ',' + str(current_index)
                key2 = str(current_index) + ',' + str(i)
                interest = 0
                if key1 in self.memo:
                    interest = self.memo[key1]
                elif key2 in self.memo:
                    interest = self.memo[key2]
                else:
                    tag_set = self.get_tag_set(i)
                    current_tag_set = self.get_tag_set(current_index)
                    interest = self.calc_interest_factor(tag_set, current_tag_set)
                    memo_key1 = str(i) + ',' + str(current_index)
                    memo_key2 = str(current_index) + ',' + str(i)
                    self.memo[memo_key1] = interest
                if interest > 0:
                    obj = heapq_object(obj=current_obj)
                    obj.add(i, self.index_to_photoIDs(i))
                    hq.heappush(self.min_heap, (current_val - interest, self.heapq_id, obj))
                    self.heapq_id = self.heapq_id + 1
                    self.update_max(-(current_val - interest), obj.index_list)
            del current_obj

    def get_tag_set(self, index):
        if index < self.num_H:
            return self.H_sets[index]
        else:
            index = index - self.num_H
            v1, v2 = self.V_pairs[index]
            return self.V_sets[v1].union(self.V_sets[v2])

    def update_max(self, interest_factor, index_list):
        if interest_factor > self.max_val:
            self.max_val = interest_factor
            self.max_list = index_list

    def index_to_photoIDs(self, index):
        ret_set = set()
        if index < self.num_H:
            ret_set.add(self.H_ID_mapping[index])
        else:
            index = index - self.num_H
            idx1, idx2 = self.V_pairs[index]
            ret_set.add(self.V_ID_mapping[idx1])
            ret_set.add(self.V_ID_mapping[idx2])
        return ret_set

    def print_solution(self):
        print('Total interest factor: ', self.max_val)
        if self.max_val == 0:
            print(1)
            print(0)
            return
        print(len(self.max_list))
        for i in self.max_list:
            id_set = self.index_to_photoIDs(self.max_list[i])
            str1 = ''
            for photo_id in id_set:
                str1 += str(photo_id) + ' '
            print(str1.rstrip())

    def print(self):
        print('H_sets', str(self.H_sets))
        print('V_sets', str(self.V_sets))
        print('V_pairs', str(self.V_pairs))
        print('tag_index_dict', str(self.tag_index_dict))
        print('H_ID_mapping', str(self.H_ID_mapping))
        print('V_ID_mapping', str(self.V_ID_mapping))

class heapq_object():
    def __init__(self, index_list=deque([]), id_set=set(), obj=None):
        if obj == None:
            self.index_list = index_list
            self.id_set = id_set
        else:
            self.index_list = deque(obj.index_list)
            self.id_set = set(obj.id_set)

    def add(self, idx, id_set):
        self.index_list.append(idx)
        self.id_set = self.id_set.union(id_set)

if __name__ == "__main__":
    input_file = input("Enter input file: ")
    soln = Solution(input_file)
    soln.load_data()

